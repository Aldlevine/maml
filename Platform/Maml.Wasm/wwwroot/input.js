import { wasm } from "./wasm.js";
function makeOutput(id = "") {
    var output = document.createElement("div");
    output.className = "output";
    output.id = id;
    output.innerHTML = `[${id}]`;
    document.getElementById("out").append(output);
    return output;
}
class Input {
    constructor() {
        this.Outputs = {
            WindowResize: makeOutput("out-window-resize"),
            PointerMove: makeOutput("out-pointer-move"),
            PointerButton: makeOutput("out-pointer-button"),
            Wheel: makeOutput("out-wheel"),
            Keyboard: makeOutput("out-keyboard"),
            Focus: makeOutput("out-focus")
        };
    }
    async init() {
        wasm.setModuleImports("input.js", this);
        const inputInterop = await wasm.getAssemblyExports("Maml.Wasm", "Maml.UserInput.Input");
        window.onpointermove = (e) => {
            e.preventDefault();
            inputInterop.HandlePointerMove(e.clientX, e.clientY, e.buttons);
        };
        window.onpointerdown = (e) => {
            e.preventDefault();
            inputInterop.HandlePointerDown(e.clientX, e.clientY, 1 << e.button, e.buttons);
        };
        window.onpointerup = (e) => {
            e.preventDefault();
            inputInterop.HandlePointerUp(e.clientX, e.clientY, 1 << e.button, e.buttons);
        };
        window.ontouchstart = (e) => {
            e.preventDefault();
        };
        window.onwheel = (e) => {
            inputInterop.HandleWheel(e.clientX, e.clientY, e.deltaX, e.deltaY);
        };
        window.onkeydown = (e) => {
            inputInterop.HandleKeyDown(e.key, e.repeat);
        };
        window.onkeyup = (e) => {
            inputInterop.HandleKeyUp(e.key);
        };
        window.onfocus = (e) => {
            inputInterop.HandleFocus();
        };
        window.onblur = (e) => {
            inputInterop.HandleBlur();
        };
    }
    report(id, value) {
        this.Outputs[id].innerHTML = value;
    }
}
export const input = new Input();
//# sourceMappingURL=input.js.map