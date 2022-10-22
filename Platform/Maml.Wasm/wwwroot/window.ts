import { wasm } from "./wasm.js";

type MamlWindowInterop = {
	HandleResize: (id: number, width: number, height: number, dpi: number) => void;
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

	private canvas: HTMLCanvasElement = <HTMLCanvasElement>document.getElementById("canvas");

	public async init(): Promise<void> {
		wasm.setModuleImports("window.js", this);
		this.interop = await wasm.getAssemblyExports<MamlWindowInterop>("Maml.Wasm", "Maml.Window");

		//window.onorientationchange =
		//window.onresize = (_evt: UIEvent) => {
		//	this.interop.HandleResize(0, Math.floor(window.innerWidth * devicePixelRatio), Math.floor(window.innerHeight * devicePixelRatio), devicePixelRatio);
		//	const width = window.innerWidth;
		//	const height = window.innerHeight;
		//	this.canvas.width = width * devicePixelRatio;
		//	this.canvas.height = height * devicePixelRatio;
		//	this.canvas.style.width = `${width}px`;
		//	this.canvas.style.height = `${height}px`;
		//};
		//window.onresize(null);

		window.oncontextmenu = (evt: PointerEvent) => {
			evt.preventDefault();
		};

		window.onpointermove = (evt: PointerEvent) => {
			this.interop.HandlePointerMove(0, evt.clientX, evt.clientY, evt.button, evt.buttons);
		};

		window.onpointerdown = (evt: PointerEvent) => {
			this.interop.HandlePointerDown(0, evt.clientX, evt.clientY, evt.button, evt.buttons);
		};

		window.onpointerup = (evt: PointerEvent) => {
			this.interop.HandlePointerUp(0, evt.clientX, evt.clientY, evt.button, evt.buttons);
		};

		this.updateSize();
	}

	private width: number = -1;
	private height: number = -1;
	private dpiRatio: number = -1;
	public updateSize(force: boolean = false): void {
		if (force || this.width != window.innerWidth || this.height != window.innerHeight || this.dpiRatio != window.devicePixelRatio) {
			this.canvas.width = Math.ceil(window.innerWidth * window.devicePixelRatio);
			this.canvas.height = Math.ceil(window.innerHeight * window.devicePixelRatio);
			this.canvas.style.width = `${window.innerWidth}px`;
			this.canvas.style.height = `${window.innerHeight}px`;
		}

		this.width = window.innerWidth;
		this.height = window.innerHeight;
		this.dpiRatio = window.devicePixelRatio;

		this.interop.HandleResize(0, Math.ceil(this.width * this.dpiRatio), Math.ceil(this.height * this.dpiRatio), this.dpiRatio);
	}
}

export const mamlWindow = new MamlWindow();
