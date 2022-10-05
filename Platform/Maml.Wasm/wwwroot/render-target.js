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
class RenderTarget {
    constructor() {
        this.canvases = {};
        this.contexts = {};
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
    clear(id, color) {
        const canvas = this.canvases[id];
        const ctx = this.contexts[id];
        //ctx.save();
        ctx.resetTransform();
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        document.body.style.background = color;
        //ctx.restore();
    }
    fillRect(id, x, y, width, height, brushId) {
        const ctx = this.contexts[id];
        //ctx.save();
        ctx.fillStyle = this.brushes[brushId];
        ctx.fillRect(x, y, width, height);
        //ctx.restore();
    }
    fillEllipse(id, x, y, radiusX, radiusY, brushId) {
        const ctx = this.contexts[id];
        //ctx.save();
        ctx.fillStyle = this.brushes[brushId];
        ctx.beginPath();
        ctx.ellipse(x, y, radiusX, radiusY, 0, 0, Math.PI * 2);
        ctx.fill();
        //ctx.restore();
    }
    strokeRect(id, x, y, width, height, brushId, thickness) {
        const ctx = this.contexts[id];
        //ctx.save();
        ctx.strokeStyle = this.brushes[brushId];
        ctx.lineWidth = thickness;
        ctx.strokeRect(x, y, width, height);
        //ctx.restore();
    }
    strokeEllipse(id, x, y, radiusX, radiusY, brushId, thickness) {
        const ctx = this.contexts[id];
        //ctx.save();
        ctx.strokeStyle = this.brushes[brushId];
        ctx.lineWidth = thickness;
        ctx.beginPath();
        ctx.ellipse(x, y, radiusX, radiusY, 0, 0, Math.PI * 2);
        ctx.stroke();
        //ctx.restore();
    }
    strokeLine(id, startX, startY, endX, endY, brushId, thickness) {
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
    getTransform(id) {
        const ctx = this.contexts[id];
        const mat = ctx.getTransform();
        return mat.toFloat64Array();
    }
    setTransform(id, matrixArray) {
        const ctx = this.contexts[id];
        ctx.setTransform(DOMMatrix.fromFloat64Array(matrixArray));
    }
    makeColorBrush(id, color) {
        this.brushes[this.currentBrushId] = color;
        return this.currentBrushId++;
    }
    releaseBrush(id, brushId) {
        delete this.brushes[brushId];
    }
}
export const renderTarget = new RenderTarget();
//# sourceMappingURL=render-target.js.map