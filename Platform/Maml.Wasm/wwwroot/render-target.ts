import { wasm } from "./wasm.js";

type RenderTargetInterop = {};
type Geometry = Path2D;
type Brush = string | CanvasGradient | CanvasPattern;

enum FontStyle
{
	Normal,
	Oblique,
	Italic,
};

enum FlowDirection
{
	LeftToRight,
	RightToLeft,
};

enum WrappingMode
{
	Normal,
	None,
	Character,
	Word,
};

type Text = {
	fontName: string,
	fontSize: number,
	fontStyle: FontStyle,
	fontWeight: number,
	//flowDirection: FlowDirection,
	lineHeight: number,
	lines: string[],
};

enum WasmDrawCommand
{
	Clear,
	SetTransform,
	FillRect,
	StrokeRect,
	FillGeometry,
	StrokeGeometry,
	FillText,
};

class RenderTarget {
	private interop: RenderTargetInterop;
	private readonly canvases: { [id: number]: HTMLCanvasElement } = {};
	private readonly contexts: { [id: number]: CanvasRenderingContext2D } = {};
	private readonly textMeasurer: HTMLElement = document.getElementById("text-measurer");

	private readonly geometries: { [id: number]: Geometry } = {};
	private currentGeometryId: number = 0;
	private readonly brushes: { [id: number]: Brush } = {};
	private currentBrushId: number = 0;
	private readonly texts: { [id: number]: Text } = {};
	private currentTextId: number = 0;

	public async init(): Promise<void> {
		wasm.setModuleImports("render-target.js", this);
		this.interop = await wasm.getAssemblyExports<RenderTargetInterop>("Maml.Wasm", "Maml.RenderTarget");

		this.canvases[0] = <HTMLCanvasElement>document.getElementById("canvas");
		this.contexts[0] = this.canvases[0].getContext("2d", {
			alpha: false,
		});
	}

	private processDrawCommands(canvasId: number, commandBuffer: Float64Array): void {
		let cmdIdx = 0;
		const ctx = this.contexts[canvasId];

		while (cmdIdx < commandBuffer.length) {
			let cmd = <WasmDrawCommand>commandBuffer[cmdIdx++];
			switch (cmd) {
				case WasmDrawCommand.Clear:
					{
						const r = commandBuffer[cmdIdx++];
						const g = commandBuffer[cmdIdx++];
						const b = commandBuffer[cmdIdx++];
						const a = commandBuffer[cmdIdx++];
						this.clear(ctx, r, g, b, a);
					}
					break;

				case WasmDrawCommand.SetTransform:
					{
						const matrixArray = new Float64Array(commandBuffer.slice(cmdIdx, cmdIdx + 6));
						cmdIdx += 6;
						this.setTransform(ctx, matrixArray);
					}
					break;

				case WasmDrawCommand.FillGeometry:
					{
						const geometryId = commandBuffer[cmdIdx++];
						const brushId = commandBuffer[cmdIdx++];
						this.fillGeometry(ctx, geometryId, brushId);
					}
					break;

				case WasmDrawCommand.StrokeGeometry:
					{
						const geometryId = commandBuffer[cmdIdx++];
						const brushId = commandBuffer[cmdIdx++];
						const thickness = commandBuffer[cmdIdx++];
						this.strokeGeometry(ctx, geometryId, brushId, thickness);
					}
					break;

				case WasmDrawCommand.FillRect:
					{
						const x = commandBuffer[cmdIdx++];
						const y = commandBuffer[cmdIdx++];
						const w = commandBuffer[cmdIdx++];
						const h = commandBuffer[cmdIdx++];
						const brushId = commandBuffer[cmdIdx++];
						this.fillRect(ctx, x, y, w, h, brushId);
					}
					break;

				case WasmDrawCommand.StrokeRect:
					{
						const x = commandBuffer[cmdIdx++];
						const y = commandBuffer[cmdIdx++];
						const w = commandBuffer[cmdIdx++];
						const h = commandBuffer[cmdIdx++];
						const brushId = commandBuffer[cmdIdx++];
						const thickness = commandBuffer[cmdIdx++];
						this.strokeRect(ctx, x, y, w, h, brushId, thickness);
					}
					break;

				case WasmDrawCommand.FillText:
					{
						const textId = commandBuffer[cmdIdx++];
						const brushId = commandBuffer[cmdIdx++];
						this.fillText(ctx, textId, brushId);
					}
					break;
			}
		}
	}

	private clear(ctx: CanvasRenderingContext2D, r: number, g: number, b: number, a: number): void {
		ctx.resetTransform();
		// ctx.clearRect(0, 0, ctx.canvas.width, ctx.canvas.height);
		const color = `rgba(${r * 255},${g * 255},${b * 255},${a})`;
		ctx.fillStyle = color;
		ctx.fillRect(0, 0, ctx.canvas.width, ctx.canvas.height);
		document.body.style.background = color;
	}

	private setTransform(ctx: CanvasRenderingContext2D, matrixArray: Float64Array): void {
		const scale = new DOMMatrix([devicePixelRatio, 0, 0, devicePixelRatio, 0, 0]);
		ctx.resetTransform();
		ctx.scale(devicePixelRatio, devicePixelRatio);
		//ctx.transform(DOMMatrix.fromFloat64Array(matrixArray));
		ctx.transform(matrixArray[0], matrixArray[1], matrixArray[2], matrixArray[3], matrixArray[4], matrixArray[5]);
		//ctx.scale(devicePixelRatio, devicePixelRatio);
	}

	private fillGeometry(ctx: CanvasRenderingContext2D, geometryId: number, brushId: number): void {
		ctx.fillStyle = this.brushes[brushId];
		ctx.fill(this.geometries[geometryId]);
	}

	private strokeGeometry(ctx: CanvasRenderingContext2D, geometryId: number, brushId: number, thickness: number): void {
		ctx.strokeStyle = this.brushes[brushId];
		ctx.lineWidth = thickness;
		ctx.stroke(this.geometries[geometryId]);
	}

	private fillRect(ctx: CanvasRenderingContext2D, x: number, y: number, w: number, h: number, brushId: number): void {
		ctx.fillStyle = this.brushes[brushId];
		ctx.fillRect(x, y, w, h);
	}

	private strokeRect(ctx: CanvasRenderingContext2D, x: number, y: number, width: number, height: number, brushId: number, thickness: number): void {
		ctx.strokeStyle = this.brushes[brushId];
		ctx.lineWidth = thickness;
		ctx.strokeRect(x, y, width, height);
	}

	private fillText(ctx: CanvasRenderingContext2D, textId: number, brushId: number) {
		ctx.fillStyle = this.brushes[brushId];
		const text = this.texts[textId];

		ctx.font = `${text.fontWeight} ${text.fontSize}px "${text.fontName}"`;
		ctx.textBaseline = "top";
		const baseline = (text.lineHeight - text.fontSize) / 2;
		for (let i = 0, ii = text.lines.length; i < ii; i++) {
			ctx.fillText(text.lines[i], 0, baseline + (i * text.lineHeight));
		}
	}

	//private clear(id: number, color: string): void {
	//	const canvas = this.canvases[id];
	//	const ctx = this.contexts[id];
	//	//ctx.save();
	//	ctx.resetTransform();
	//	ctx.clearRect(0, 0, canvas.width, canvas.height);
	//	document.body.style.background = color;
	//	//ctx.restore();
	//}

	//private fillRect(id: number, x: number, y: number, width: number, height: number, brushId: number): void {
	//	const ctx = this.contexts[id];
	//	//ctx.save();
	//	ctx.fillStyle = this.brushes[brushId];
	//	ctx.fillRect(x, y, width, height);
	//	//ctx.restore();
	//}

	//private fillEllipse(id: number, x: number, y: number, radiusX: number, radiusY: number, brushId: number): void {
	//	const ctx = this.contexts[id];
	//	//ctx.save();
	//	ctx.fillStyle = this.brushes[brushId];
	//	ctx.beginPath();
	//	ctx.ellipse(x, y, radiusX, radiusY, 0, 0, Math.PI * 2);
	//	ctx.fill();
	//	//ctx.restore();
	//}

	//private strokeRect(id: number, x: number, y: number, width: number, height: number, brushId: number, thickness: number): void {
	//	const ctx = this.contexts[id];
	//	//ctx.save();
	//	ctx.strokeStyle = this.brushes[brushId];
	//	ctx.lineWidth = thickness;
	//	ctx.strokeRect(x, y, width, height);
	//	//ctx.restore();
	//}

	//private strokeEllipse(id: number, x: number, y: number, radiusX: number, radiusY: number, brushId: number, thickness: number): void {
	//	const ctx = this.contexts[id];
	//	//ctx.save();
	//	ctx.strokeStyle = this.brushes[brushId];
	//	ctx.lineWidth = thickness;
	//	ctx.beginPath();
	//	ctx.ellipse(x, y, radiusX, radiusY, 0, 0, Math.PI * 2);
	//	ctx.stroke();
	//	//ctx.restore();
	//}

	//private strokeLine(id: number, startX: number, startY: number, endX: number, endY: number, brushId: number, thickness: number): void {
	//	const ctx = this.contexts[id];
	//	//ctx.save();
	//	ctx.strokeStyle = this.brushes[brushId];
	//	ctx.lineWidth = thickness;
	//	ctx.beginPath();
	//	ctx.moveTo(startX, startY);
	//	ctx.lineTo(endX, endY);
	//	ctx.stroke();
	//	//ctx.restore();
	//}

	//private getTransform(id: number): Float64Array {
	//	const ctx = this.contexts[id];
	//	const mat = ctx.getTransform();
	//	return mat.toFloat64Array();
	//}

	//private setTransform(id: number, matrixArray: Float64Array): void {
	//	const ctx = this.contexts[id];
	//	ctx.setTransform(DOMMatrix.fromFloat64Array(matrixArray));
	//}

	//private fillGeometry(id: number, geometryId: number, brushId: number): void {
	//	const ctx = this.contexts[id];
	//	ctx.fillStyle = this.brushes[brushId];
	//	ctx.fill(this.geometries[geometryId]);
	//}

	//private strokeGeometry(id: number, geometryId: number, brushId: number, thickness: number): void {
	//	const ctx = this.contexts[id];
	//	ctx.strokeStyle = this.brushes[brushId];
	//	ctx.lineWidth = thickness;
	//	ctx.stroke(this.geometries[geometryId]);
	//}

	private releaseGeometry(id: number, geometryId: number): void {
		delete this.geometries[geometryId];
	}

	private makeRectGeometry(id: number, x: number, y: number, w: number, h: number): number {
		const path = new Path2D();
		path.rect(x, y, w, h);
		this.geometries[this.currentGeometryId] = path;
		return this.currentGeometryId++;
	}

	private makeEllipseGeometry(id: number, x: number, y: number, radiusX: number, radiusY: number): number {
		const path = new Path2D();
		path.ellipse(x, y, radiusX, radiusY, 0, 0, Math.PI * 2);
		this.geometries[this.currentGeometryId] = path;
		return this.currentGeometryId++;
	}

	private makeLineGeometry(id: number, startX: number, startY: number, endX: number, endY: number): number {
		const path = new Path2D();
		path.moveTo(startX, startY);
		path.lineTo(endX, endY);
		this.geometries[this.currentGeometryId] = path;
		return this.currentGeometryId++;
	}

	private releaseBrush(id: number, brushId: number): void {
		delete this.brushes[brushId];
	}

	private makeColorBrush(id: number, color: string): number {
		this.brushes[this.currentBrushId] = color;
		return this.currentBrushId++;
	}

	private releaseText(id: number, textId: number): void {
		delete this.texts[textId];
	}

	private makeText(
		id: number,
		text: string,
		wrappingMode: WrappingMode,
		lineHeight: number,
		fontName: string,
		fontSize: number,
		fontStyle: FontStyle,
		fontWeight: number,
		maxSizeX: number,
		maxSizeY: number,
	): Float64Array {
		//(<any>this.textMeasurer.style).zoom = 1 / devicePixelRatio;
		this.textMeasurer.style.font = `${fontWeight} ${fontSize}px "${fontName}"`;

		this.textMeasurer.innerText = " ";
		const spaceWidth = this.textMeasurer.getClientRects()[0].width;

		switch (wrappingMode) {
			case WrappingMode.None:
				this.textMeasurer.className = "none";
				break;
			case WrappingMode.Normal:
				this.textMeasurer.className = "normal";
				break;
			case WrappingMode.Word:
				this.textMeasurer.className = "word";
				break;
			case WrappingMode.Character:
				this.textMeasurer.className = "character";
				// insert zero-width space between each character
				text = [...text].join("\u200b");
				break;
		}
		
		this.textMeasurer.style.width = `${maxSizeX}px`;
		this.textMeasurer.innerText = text;
		for (let textNode of <any>this.textMeasurer.childNodes) {
			if (!(textNode instanceof Text)) { continue; }
			while (textNode.length > 1) {
				textNode.splitText(textNode.length - 1);
			}
		}

		const range = document.createRange();
		range.selectNodeContents(this.textMeasurer);
		
		const lines = [];
		let curLine = "";
		let lastTop = 0;
		let lastRight = 0;
		let width = 0;
		for (let t of <any>this.textMeasurer.childNodes) {
			range.selectNode(t);
			const rect = range.getClientRects()[0];
			width = Math.max(rect.right, width);
			if (rect.top > lastTop) {
				// new line
				if (curLine != "") {
					lines.push(curLine);
				}
				curLine = t.textContent;
			}
			else {
				if (rect.left > lastRight) {
					if (spaceWidth > 0) {
						const numSpaces = Math.floor((rect.left - lastRight) / spaceWidth);
						const spaces = Array(numSpaces).fill(" ").join("");
						curLine += spaces;
					}
				}
				curLine += t.textContent;
			}
			
			lastTop = rect.top;
			lastRight = rect.right;
		}
		if (curLine != "") {
			lines.push(curLine);
		}

		const textObj: Text = {
			fontName,
			fontSize,
			fontStyle,
			fontWeight,
			lineHeight,
			lines,
		};
		this.texts[this.currentTextId] = textObj;
		const height = lines.length * lineHeight;

		return new Float64Array([this.currentTextId++, lines.length, width, height]);
	
	}
}

export const renderTarget = new RenderTarget();
