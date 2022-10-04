import { wasm } from "./wasm.js";

type MamlWindowInterop = {
	HandleResize: (id: number, width: number, height: number) => void;
	HandlePointerMove: (id: number, x: number, y: number, button: number, buttonMask: number) => void;
	HandlePointerDown: (id: number, x: number, y: number, button: number, buttonMask: number) => void;
	HandlePointerUp: (id: number, x: number, y: number, button: number, buttonMask: number) => void;
//	HandleWheel: (x: number, y: number, dx: number, dy: number) => void;
//	HandleKeyDown: (key: string, echo: boolean) => void;
//	HandleKeyUp: (key: string) => void;
//	HandleFocus: () => void;
//	HandleBlur: () => void;
}

class MamlWindow {
	public async init(): Promise<void> {
		wasm.setModuleImports("window.js", this);

		const interop = await wasm.getAssemblyExports<MamlWindowInterop>("Maml.Wasm", "Maml.Window");

		window.onresize = (ev: UIEvent) => {
			interop.HandleResize(0, window.innerWidth, window.innerHeight);
		};

		window.onpointermove = (ev: PointerEvent) => {
			interop.HandlePointerMove(0, ev.clientX, ev.clientY, ev.button, ev.buttons);
		};

		window.onpointerdown = (ev: PointerEvent) => {
			interop.HandlePointerDown(0, ev.clientX, ev.clientY, ev.button, ev.buttons);
		};

		window.onpointerup = (ev: PointerEvent) => {
			interop.HandlePointerUp(0, ev.clientX, ev.clientY, ev.button, ev.buttons);
		};
	}
}

export const mamlWindow = new MamlWindow();
