import { wasm } from "./wasm.js";

type RenderTargetInterop = {

}

class RenderTarget {
	private interop: RenderTargetInterop;
	private readonly canvases: { [id: number]: HTMLCanvasElement } = {};
	private readonly contexts: { [id: number]: CanvasRenderingContext2D } = {};

	public async init(): Promise<void> {
		wasm.setModuleImports("render-target.js", this);
		this.interop = await wasm.getAssemblyExports<RenderTargetInterop>("Maml.Wasm", "Maml.RenderTarget");

		this.canvases[0] = <HTMLCanvasElement>document.getElementById("canvas");
		this.contexts[0] = this.canvases[0].getContext("2d", {
			alpha: true,
		});
	}

	private clear(id: number, color: string) {
		const canvas = this.canvases[id];
		this.contexts[id].clearRect(0, 0, canvas.width, canvas.height);
		document.body.style.background = color;
	}
}

export const renderTarget = new RenderTarget();
