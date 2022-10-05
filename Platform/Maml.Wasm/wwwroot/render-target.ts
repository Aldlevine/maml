import { wasm } from "./wasm.js";

type RenderTargetInterop = {};
type Brush = string | CanvasGradient | CanvasPattern;

class RenderTarget {
	private interop: RenderTargetInterop;
	private readonly canvases: { [id: number]: HTMLCanvasElement } = {};
	private readonly contexts: { [id: number]: CanvasRenderingContext2D } = {};
	private readonly brushes: { [id: number]: Brush } = {};
	private currentBrushId: number = 0;

	public async init(): Promise<void> {
		wasm.setModuleImports("render-target.js", this);
		this.interop = await wasm.getAssemblyExports<RenderTargetInterop>("Maml.Wasm", "Maml.RenderTarget");

		this.canvases[0] = <HTMLCanvasElement>document.getElementById("canvas");
		this.contexts[0] = this.canvases[0].getContext("2d", {
			alpha: true,
		});
	}

	private clear(id: number, color: string): void {
		const canvas = this.canvases[id];
		const ctx = this.contexts[id];
		//ctx.save();
		ctx.resetTransform();
		ctx.clearRect(0, 0, canvas.width, canvas.height);
		document.body.style.background = color;
		//ctx.restore();
	}

	private fillRect(id: number, x: number, y: number, width: number, height: number, brushId: number): void {
		const ctx = this.contexts[id];
		//ctx.save();
		ctx.fillStyle = this.brushes[brushId];
		ctx.fillRect(x, y, width, height);
		//ctx.restore();
	}

	private fillEllipse(id: number, x: number, y: number, radiusX: number, radiusY: number, brushId: number): void {
		const ctx = this.contexts[id];
		//ctx.save();
		ctx.fillStyle = this.brushes[brushId];
		ctx.beginPath();
		ctx.ellipse(x, y, radiusX, radiusY, 0, 0, Math.PI * 2);
		ctx.fill();
		//ctx.restore();
	}

	private strokeRect(id: number, x: number, y: number, width: number, height: number, brushId: number, thickness: number): void {
		const ctx = this.contexts[id];
		//ctx.save();
		ctx.strokeStyle = this.brushes[brushId];
		ctx.lineWidth = thickness;
		ctx.strokeRect(x, y, width, height);
		//ctx.restore();
	}

	private strokeEllipse(id: number, x: number, y: number, radiusX: number, radiusY: number, brushId: number, thickness: number): void {
		const ctx = this.contexts[id];
		//ctx.save();
		ctx.strokeStyle = this.brushes[brushId];
		ctx.lineWidth = thickness;
		ctx.beginPath();
		ctx.ellipse(x, y, radiusX, radiusY, 0, 0, Math.PI * 2);
		ctx.stroke();
		//ctx.restore();
	}

	private strokeLine(id: number, startX: number, startY: number, endX: number, endY: number, brushId: number, thickness: number): void {
		const ctx = this.contexts[id];
		//ctx.save();
		ctx.strokeStyle = this.brushes[brushId];
		ctx.lineWidth = thickness;
		ctx.beginPath();
		ctx.moveTo(startX, startY);
		ctx.lineTo(endX, endY);
		ctx.stroke();
		//ctx.restore();
	}

	private getTransform(id: number): Float64Array {
		const ctx = this.contexts[id];
		const mat = ctx.getTransform();
		return mat.toFloat64Array();
	}

	private setTransform(id: number, matrixArray: Float64Array): void {
		const ctx = this.contexts[id];
		ctx.setTransform(DOMMatrix.fromFloat64Array(matrixArray));
	}

	private makeColorBrush(id: number, color: string): number {
		this.brushes[this.currentBrushId] = color;
		return this.currentBrushId++;
	}

	private releaseBrush(id: number, brushId: number): void {
		delete this.brushes[brushId];
	}
}

export const renderTarget = new RenderTarget();
