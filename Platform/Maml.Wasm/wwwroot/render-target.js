var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import { wasm } from "./wasm.js";
var WasmDrawCommand;
(function (WasmDrawCommand) {
    WasmDrawCommand[WasmDrawCommand["Clear"] = 0] = "Clear";
    WasmDrawCommand[WasmDrawCommand["SetTransform"] = 1] = "SetTransform";
    WasmDrawCommand[WasmDrawCommand["FillRect"] = 2] = "FillRect";
    WasmDrawCommand[WasmDrawCommand["StrokeRect"] = 3] = "StrokeRect";
    WasmDrawCommand[WasmDrawCommand["FillGeometry"] = 4] = "FillGeometry";
    WasmDrawCommand[WasmDrawCommand["StrokeGeometry"] = 5] = "StrokeGeometry";
})(WasmDrawCommand || (WasmDrawCommand = {}));
;
class RenderTarget {
    constructor() {
        this.canvases = {};
        this.contexts = {};
        this.geometries = {};
        this.currentGeometryId = 0;
        this.brushes = {};
        this.currentBrushId = 0;
    }
    init() {
        return __awaiter(this, void 0, void 0, function* () {
            wasm.setModuleImports("render-target.js", this);
            this.interop = yield wasm.getAssemblyExports("Maml.Wasm", "Maml.RenderTarget");
            this.canvases[0] = document.getElementById("canvas");
            this.contexts[0] = this.canvases[0].getContext("2d", {
                alpha: true,
            });
        });
    }
    processDrawCommands(canvasId, commandBuffer) {
        let cmdIdx = 0;
        const ctx = this.contexts[canvasId];
        while (cmdIdx < commandBuffer.length) {
            let cmd = commandBuffer[cmdIdx++];
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
                case WasmDrawCommand.SetTransform:
                    {
                        // const matrixArray = new Float64Array(commandBuffer, cmdIdx * 8, 6);
                        // const matrixArray = new Float64Array(commandBuffer, cmdIdx * 8, 6);
                        const matrixArray = new Float64Array(commandBuffer.slice(cmdIdx, cmdIdx + 6));
                        cmdIdx += 6;
                        this.setTransform(ctx, matrixArray);
                    }
                    break;
            }
        }
    }
    clear(ctx, r, g, b, a) {
        ctx.resetTransform();
        ctx.clearRect(0, 0, ctx.canvas.width, ctx.canvas.height);
        document.body.style.background = `rgba(${r * 255},${g * 255},${b * 255},${a})`;
    }
    fillGeometry(ctx, geometryId, brushId) {
        ctx.fillStyle = this.brushes[brushId];
        ctx.fill(this.geometries[geometryId]);
    }
    strokeGeometry(ctx, geometryId, brushId, thickness) {
        ctx.strokeStyle = this.brushes[brushId];
        ctx.lineWidth = thickness;
        ctx.stroke(this.geometries[geometryId]);
    }
    fillRect(ctx, x, y, w, h, brushId) {
        ctx.fillStyle = this.brushes[brushId];
        ctx.fillRect(x, y, w, h);
    }
    strokeRect(ctx, x, y, width, height, brushId, thickness) {
        ctx.strokeStyle = this.brushes[brushId];
        ctx.lineWidth = thickness;
        ctx.strokeRect(x, y, width, height);
    }
    setTransform(ctx, matrixArray) {
        ctx.setTransform(DOMMatrix.fromFloat64Array(matrixArray));
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
    releaseGeometry(id, geometryId) {
        delete this.geometries[geometryId];
    }
    makeRectGeometry(id, x, y, w, h) {
        const path = new Path2D();
        path.rect(x, y, w, h);
        this.geometries[this.currentGeometryId] = path;
        return this.currentGeometryId++;
    }
    makeEllipseGeometry(id, x, y, radiusX, radiusY) {
        const path = new Path2D();
        path.ellipse(x, y, radiusX, radiusY, 0, 0, Math.PI * 2);
        this.geometries[this.currentGeometryId] = path;
        return this.currentGeometryId++;
    }
    makeLineGeometry(id, startX, startY, endX, endY) {
        const path = new Path2D();
        path.moveTo(startX, startY);
        path.lineTo(endX, endY);
        this.geometries[this.currentGeometryId] = path;
        return this.currentGeometryId++;
    }
    releaseBrush(id, brushId) {
        delete this.brushes[brushId];
    }
    makeColorBrush(id, color) {
        this.brushes[this.currentBrushId] = color;
        return this.currentBrushId++;
    }
}
export const renderTarget = new RenderTarget();
//# sourceMappingURL=render-target.js.map