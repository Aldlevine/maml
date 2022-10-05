import { wasm } from "./wasm.js";

type MamlWindowInterop = {
	HandleResize: (id: number, width: number, height: number) => void;
	HandlePointerMove: (id: number, x: number, y: number, iButton: number, iButtonMask: number) => void;
	HandlePointerDown: (id: number, x: number, y: number, iButton: number, iButtonMask: number) => void;
	HandlePointerUp: (id: number, x: number, y: number, iButton: number, iButtonMask: number) => void;
//	HandleWheel: (x: number, y: number, dx: number, dy: number) => void;
//	HandleKeyDown: (key: string, echo: boolean) => void;
//	HandleKeyUp: (key: string) => void;
//	HandleFocus: () => void;
//	HandleBlur: () => void;
}

class MamlWindow {
	private interop: MamlWindowInterop;

	public async init(): Promise<void> {
		wasm.setModuleImports("window.js", this);
		this.interop = await wasm.getAssemblyExports<MamlWindowInterop>("Maml.Wasm", "Maml.Window");

		window.onorientationchange =
		window.onresize = (_evt: UIEvent) => {
			this.interop.HandleResize(0, window.innerWidth, window.innerHeight);
			const canvas = <HTMLCanvasElement>document.getElementById("canvas");
			canvas.width = window.innerWidth;
			canvas.height = window.innerHeight;
			canvas.style.width = `${window.innerWidth}px`;
			canvas.style.height = `${window.innerHeight}px`;
		};
		window.onresize(null);

		window.onpointermove = (evt: PointerEvent) => {
			this.interop.HandlePointerMove(0, evt.clientX, evt.clientY, evt.button, evt.buttons);
		};

		window.onpointerdown = (evt: PointerEvent) => {
			this.interop.HandlePointerDown(0, evt.clientX, evt.clientY, evt.button, evt.buttons);
		};

		window.onpointerup = (evt: PointerEvent) => {
			this.interop.HandlePointerUp(0, evt.clientX, evt.clientY, evt.button, evt.buttons);
		};
	}
}

export const mamlWindow = new MamlWindow();
