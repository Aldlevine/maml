import { wasm } from "./wasm.js";

type RendererWasm = {
	HandleResize: (width: number, height: number) => void;
	HandleInit: (width: number, height: number) => void;
}

function colorIntToHex(color: number) {
	return "#"+Math.round(color).toString(16).padStart(8, "0");
}

class Renderer {
	private canvas: HTMLCanvasElement = <HTMLCanvasElement>document.getElementById("canvas");
	private ctx: CanvasRenderingContext2D = this.canvas.getContext("2d", {
		alpha: true,
		desynchronized: false
	});
	private renderer: RendererWasm;

	public async init(): Promise<void> {
		wasm.setModuleImports("renderer.js", this);
		this.renderer = await wasm.getAssemblyExports<RendererWasm>("Maml.Wasm", "Maml.Drawing.Renderer");

		window.onresize = window.onorientationchange = () => this.resize();
		this.resize();
	}

	public resize(): void {
		let width: number = Math.ceil(window.innerWidth / 2) * 2;
		let height: number = Math.ceil(window.innerHeight / 2) * 2;
		this.canvas.width = Math.ceil(width * devicePixelRatio);
		this.canvas.height = Math.ceil(height * devicePixelRatio);
		this.canvas.style.width = `${width}px`;
		this.canvas.style.height = `${height}px`;
		this.renderer.HandleResize(window.innerWidth, window.innerHeight);
	}

	public beginDraw() {
		this.ctx.resetTransform();
		this.ctx.translate(-0.5, -0.5);
	}
	public endDraw() { }

	public clearRect(x: number, y: number, w: number, h: number): void {
		this.ctx.clearRect(x * devicePixelRatio, y * devicePixelRatio, w * devicePixelRatio, h * devicePixelRatio);
	}

	// public pushClip(Path path) { }
	// public popClip() { }

	public setTransform(xx: number, xy: number, yx: number, yy: number, tx: number, ty: number): void {
		this.ctx.resetTransform();
		this.ctx.translate(-0.5, -0.5);
		this.ctx.scale(devicePixelRatio, devicePixelRatio);
		this.ctx.transform(xx, xy, yx, yy, tx, ty);
	}

	public fillPath(id: number): void {
		this.ctx.fill(this.paths[id]);
	}
	public strokePath(id: number): void {
		this.ctx.stroke(this.paths[id]);
	}

	public setFillBrush(id: number): void {
		this.ctx.fillStyle = this.brushes[id];
	}
	public setStrokeBrush(id: number): void {
		this.ctx.strokeStyle = this.brushes[id];
	}

	// Path Loading
	private paths: { [key: number]: Path2D } = {};

	public path_new(id: number): void {
		this.paths[id] = new Path2D();
	}

	public path_arc(id: number, x: number, y: number, radius: number, startAngle: number, endAngle: number): void {
		this.paths[id].arc(x, y, radius, startAngle, endAngle);
	}

	public path_arcTo(id: number, x1: number, y1: number, x2: number, y2: number, radius: number): void {
		this.paths[id].arcTo(x1, y1, x2, y2, radius);
	}

	public path_quadraticCurve(id: number, x1: number, y1: number, cpx: number, cpy: number, x2: number, y2: number): void {
		this.paths[id].moveTo(x1, y1);
		this.paths[id].quadraticCurveTo(cpx, cpy, x2, y2);
	}

	public path_bezierCurve(id: number, x1: number, y1: number, cpx1: number, cpy1: number, cpx2: number, cpy2: number, x2: number, y2: number): void {
		this.paths[id].moveTo(x1, y1);
		this.paths[id].bezierCurveTo(cpx1, cpy1, cpx2, cpy2, x2, y2);
	}

	public path_ellipse(id: number, x: number, y: number, rx: number, ry: number, rotation: number, startAngle: number, endAngle: number): void {
		this.paths[id].ellipse(x, y, rx, ry, rotation, startAngle, endAngle);
	}

	public path_line(id: number, x1: number, y1: number, x2: number, y2: number): void {
		this.paths[id].moveTo(x1, x2);
		this.paths[id].lineTo(x2, y2);
	}

	public path_rect(id: number, x: number, y: number, w: number, h: number): void {
		this.paths[id].rect(x, y, w, h);
	}

	// Brush Loading
	private brushes: { [key: number]: string | CanvasGradient | CanvasPattern } = {};

	public brush_color_new(id: number, color: number): void {
		this.brushes[id] = colorIntToHex(color);
	}

	public brush_linearGradient_new(id: number, x1: number, y1: number, x2: number, y2: number): void {
		this.brushes[id] = this.ctx.createLinearGradient(x1, y1, x2, y2);
	}

	public brush_addColorStop(id: number, offset: number, color: number) {
		(<CanvasGradient>this.brushes[id]).addColorStop(offset, colorIntToHex(color));
	}

}

export const renderer = new Renderer();