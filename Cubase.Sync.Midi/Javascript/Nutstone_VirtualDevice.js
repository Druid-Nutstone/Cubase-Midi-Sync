var midiremote = require('midiremote_api_v1')

// create the device driver main object
var deviceDriver = midiremote.makeDeviceDriver('Nutstone', 'VirtualDevice', 'David Nuttall')

// create objects representing the hardware's MIDI ports
var midiInput = deviceDriver.mPorts.makeMidiInput()
var midiOutput = deviceDriver.mPorts.makeMidiOutput()

// define all possible namings the devices MIDI ports could have
deviceDriver.makeDetectionUnit().detectPortPair(midiInput, midiOutput)
    .expectInputNameEquals('Nutstone IN')
    .expectOutputNameEquals('Nutstone OUT')

deviceDriver.makeDetectionUnit().detectPortPair(midiInput, midiOutput)
    .expectInputNameEquals('Nutstone IN (MIDI IN')
    .expectOutputNameEquals('Nutstone OUT (MIDI OUT')

var pages = {};
var activePage = null;

// Special CC to switch pages dynamically
var PAGE_SELECT_CC = 0; // pick an unused CC number

function getNested(obj, path) {
    var parts = path.split('.');
    var current = obj;

    for (var i = 0; i < parts.length; i++) {
        if (!current || current[parts[i]] === undefined) {
            return null;
        }
        current = current[parts[i]];
    }
    return current;
}


function loadPage(pageData) {
    try {
        if (!pageData || !pageData.name) {
            console.log("Skipping invalid page data");
            return;
        }

        var page = deviceDriver.mMapping.makePage(pageData.name);
        console.log("Creating page: " + pageData.name);

        var buttons = {}, faders = {}, knobs = {};

        // Buttons
        if (Array.isArray(pageData.buttons)) {
            pageData.buttons.forEach(function (b) {
                try {
                    if (!b.name || !b.command || b.command.length < 2) return;
                    var btn = deviceDriver.mSurface.makeButton(1, 1, 1, 1);
                    page.makeCommandBinding(btn.mSurfaceValue, b.command[0], b.command[1]);
                    buttons[b.cc] = btn.mSurfaceValue;
                } catch (e) {
                    console.log("Button error: " + (b.name || "unknown") + " -> " + e);
                }
            });
        }

        // Faders
        if (Array.isArray(pageData.faders)) {
            pageData.faders.forEach(function (f) {
                try {
                    if (!f.name || !f.parameter) return;
                    var fader = deviceDriver.mSurface.makeKnob(1, 1, 1, 1);
                    fader.mSurfaceValue.mMidiBinding.setInputPort(midiInput).bindToControlChange(0, f.cc);

                    var param = getNested(page.mHostAccess, f.parameter);
                    if (param) page.makeValueBinding(fader.mSurfaceValue, param);
                    faders[f.cc] = fader.mSurfaceValue;
                } catch (e) {
                    console.log("Fader error: " + (f.name || "unknown") + " -> " + e);
                }
            });
        }

        // Knobs
        if (Array.isArray(pageData.knobs)) {
            pageData.knobs.forEach(function (k) {
                try {
                    if (!k.name || !k.parameter) return;
                    var knob = deviceDriver.mSurface.makeKnob(1, 1, 1, 1);
                    knob.mSurfaceValue.mMidiBinding.setInputPort(midiInput).bindToControlChange(0, k.cc);
                    var param = getNested(page.mHostAccess, k.parameter);
                    if (param) page.makeValueBinding(knob.mSurfaceValue, param);
                    knobs[k.cc] = knob.mSurfaceValue;
                } catch (e) {
                    console.log("Knob error: " + (k.name || "unknown") + " -> " + e);
                }
            });
        }

        pages[pageData.name] = { page, buttons, faders, knobs };

    } catch (e) {
        console.log("Error in loadPage('" + (pageData.name || "unknown") + "'): " + e);
    }
}

midiInput.mOnSysex = function (activeDevice, message) {
    var data = message.mData;

    if (!data || data.length < 3) return;
    if (data[0] !== 0xF0 || data[data.length - 1] !== 0xF7) return;

    var cmd = data[2];



    function extractNameAndValue(data, startIndex) {
        var endIndex = -1;
        for (var i = startIndex; i < data.length; i++) {
            if (data[i] === 0x00) {
                endIndex = i;
                break;
            }
        }
        if (endIndex < 0) return null;

        var name = "";
        for (var i = startIndex; i < endIndex; i++) {
            name += String.fromCharCode(data[i]);
        }

        var value = 0;
        if (endIndex + 1 < data.length) {
            value = data[endIndex + 1];
        }

        return { name: name, value: value };
    }

    switch (cmd) {
        case 0x01: // Switch page
            var result = extractNameAndValue(data, 3);
            if (result && pages[result.name]) {
                activePage = pages[result.name];
            }
            break;

        case 0x02: // Button
            var result = extractNameAndValue(data, 3);
            if (result && activePage && activePage.buttons && activePage.buttons[result.name]) {
                activePage.buttons[result.name].setProcessValue(activeDevice, result.value > 0 ? 1 : 0);
            }
            break;

        case 0x03: // Fader
            var result = extractNameAndValue(data, 3);
            if (result && activePage && activePage.faders && activePage.faders[result.name]) {
                activePage.faders[result.name].setProcessValue(activeDevice, result.value / 127);
            }
            break;

        case 0x04: // Knob
            var result = extractNameAndValue(data, 3);
            if (result && activePage && activePage.knobs && activePage.knobs[result.name]) {
                activePage.knobs[result.name].setProcessValue(activeDevice, result.value / 127);
            }
            break;

        case 0x10: // JSON mapping
            var jsonStr = "";
            for (var i = 3; i < data.length - 1; i++) {
                jsonStr += String.fromCharCode(data[i]);
            }
            var mapping = JSON.parse(jsonStr);

            pages = {};
            activePage = null;

            for (var i = 0; i < mapping.pages.length; i++) {
                loadPage(mapping.pages[i]);
            }
            break;
    }
};









