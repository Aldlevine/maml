import { wasm } from "./wasm.js";

type InputInterop = {
	HandlePointerMove: (x: number, y: number, buttonMask: number) => void;
	HandlePointerDown: (x: number, y: number, button: number, buttonMask: number) => void;
	HandlePointerUp: (x: number, y: number, button: number, buttonMask: number) => void;
	HandleWheel: (x: number, y: number, dx: number, dy: number) => void;
	HandleKeyDown: (key: string, echo: boolean) => void;
	HandleKeyUp: (key: string) => void;
	HandleFocus: () => void;
	HandleBlur: () => void;
}

function makeOutput(id: string = ""): HTMLElement {
	var output = document.createElement("div");
	output.className = "output";
	output.id = id;
	output.innerHTML = `[${id}]`;
	document.getElementById("out").append(output);
	return output;
}

class Input {
	public async init() {
		wasm.setModuleImports("input.js", this);

		const inputInterop = await wasm.getAssemblyExports<InputInterop>("Maml.Wasm", "Maml.UserInput.Input");

		window.onpointermove = (e: PointerEvent) => {
			e.preventDefault();
			inputInterop.HandlePointerMove(e.clientX, e.clientY, e.buttons);
		}

		window.onpointerdown = (e: PointerEvent) => {
			e.preventDefault();
			inputInterop.HandlePointerDown(e.clientX, e.clientY, 1 << e.button, e.buttons);
		}

		window.onpointerup = (e: PointerEvent) => {
			e.preventDefault();
			inputInterop.HandlePointerUp(e.clientX, e.clientY, 1 << e.button, e.buttons);
		}

		window.ontouchstart = (e: TouchEvent) => {
			e.preventDefault();
		}

		window.onwheel = (e: WheelEvent) => {
			inputInterop.HandleWheel(e.clientX, e.clientY, e.deltaX, e.deltaY);
		}

		window.onkeydown = (e: KeyboardEvent) => {
			inputInterop.HandleKeyDown(e.key, e.repeat);
		}

		window.onkeyup = (e: KeyboardEvent) => {
			inputInterop.HandleKeyUp(e.key);
		}

		window.onfocus = (e: FocusEvent) => {
			inputInterop.HandleFocus();
		}

		window.onblur = (e: FocusEvent) => {
			inputInterop.HandleBlur();
		}
	}

	private readonly Outputs: {[key: string]: HTMLElement} = {
		WindowResize: makeOutput("out-window-resize"),
		PointerMove: makeOutput("out-pointer-move"),
		PointerButton: makeOutput("out-pointer-button"),
		Wheel: makeOutput("out-wheel"),
		Keyboard: makeOutput("out-keyboard"),
		Focus: makeOutput("out-focus")
	};

	private report(id: string, value: string): void {
		this.Outputs[id].innerHTML = value;
	}
}

export const input = new Input();