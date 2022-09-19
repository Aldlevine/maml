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
        const input = await wasm.getAssemblyExports("Maml.Wasm", "Maml.UserInput.Input");
        window.onscroll = (e) => {
            e.preventDefault();
            window.scrollTo(0, 0);
        };
        window.onpointermove = (e) => {
            e.preventDefault();
            input.HandlePointerMove(e.clientX, e.clientY, e.buttons);
        };
        window.onpointerdown = (e) => {
            e.preventDefault();
            input.HandlePointerDown(e.clientX, e.clientY, 1 << e.button, e.buttons);
        };
        window.onpointerup = (e) => {
            e.preventDefault();
            input.HandlePointerUp(e.clientX, e.clientY, 1 << e.button, e.buttons);
        };
        window.ontouchstart = (e) => {
            e.preventDefault();
        };
        window.onwheel = (e) => {
            input.HandleWheel(e.clientX, e.clientY, e.deltaX, e.deltaY);
        };
        window.onkeydown = (e) => {
            input.HandleKeyDown(e.key, e.repeat);
        };
        window.onkeyup = (e) => {
            input.HandleKeyUp(e.key);
        };
        window.onfocus = (e) => {
            input.HandleFocus();
        };
        window.onblur = (e) => {
            input.HandleBlur();
        };
    }
    report(id, value) {
        this.Outputs[id].innerHTML = value;
    }
}
export const input = new Input();
//# sourceMappingURL=input.js.map