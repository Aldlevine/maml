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
function colorIntToHex(color) {
    return "#" + Math.round(color).toString(16).padStart(8, "0");
}
class Viewport {
    constructor() {
        this.canvas = document.getElementById("canvas");
        this.ctx = this.canvas.getContext("2d", {
            alpha: true,
            desynchronized: false
        });
        // Path Loading
        this.paths = {};
        // Brush Loading
        this.brushes = {};
    }
    init() {
        return __awaiter(this, void 0, void 0, function* () {
            wasm.setModuleImports("viewport.js", this);
            this.viewportInterop = yield wasm.getAssemblyExports("Maml.Wasm", "Maml.Graphics.Viewport");
            window.onresize = window.onorientationchange = () => this.resize();
            this.resize();
        });
    }
    resize() {
        let width = Math.ceil(window.innerWidth / 2) * 2 + 1;
        let height = Math.ceil(window.innerHeight / 2) * 2 + 1;
        this.canvas.width = Math.ceil(width * devicePixelRatio);
        this.canvas.height = Math.ceil(height * devicePixelRatio);
        this.canvas.style.width = `${width}px`;
        this.canvas.style.height = `${height}px`;
        this.viewportInterop.HandleResize(width, height);
    }
    beginDraw() {
        this.ctx.resetTransform();
        this.ctx.translate(-0.5, -0.5);
    }
    endDraw() { }
    clearRect(x, y, w, h) {
        this.ctx.clearRect(x * devicePixelRatio, y * devicePixelRatio, w * devicePixelRatio, h * devicePixelRatio);
    }
    // public pushClip(Path path) { }
    // public popClip() { }
    setTransform(xx, xy, yx, yy, tx, ty) {
        this.ctx.resetTransform();
        this.ctx.translate(-0.5, -0.5);
        this.ctx.scale(devicePixelRatio, devicePixelRatio);
        this.ctx.transform(xx, xy, yx, yy, tx, ty);
    }
    fillPath(id) {
        this.ctx.fill(this.paths[id]);
    }
    strokePath(id) {
        this.ctx.stroke(this.paths[id]);
    }
    setFillBrush(id) {
        this.ctx.fillStyle = this.brushes[id];
    }
    setStrokeBrush(id) {
        this.ctx.strokeStyle = this.brushes[id];
    }
    path_new(id) {
        this.paths[id] = new Path2D();
    }
    path_arc(id, x, y, radius, startAngle, endAngle) {
        this.paths[id].arc(x, y, radius, startAngle, endAngle);
    }
    path_arcTo(id, x1, y1, x2, y2, radius) {
        this.paths[id].arcTo(x1, y1, x2, y2, radius);
    }
    path_quadraticCurve(id, x1, y1, cpx, cpy, x2, y2) {
        this.paths[id].moveTo(x1, y1);
        this.paths[id].quadraticCurveTo(cpx, cpy, x2, y2);
    }
    path_bezierCurve(id, x1, y1, cpx1, cpy1, cpx2, cpy2, x2, y2) {
        this.paths[id].moveTo(x1, y1);
        this.paths[id].bezierCurveTo(cpx1, cpy1, cpx2, cpy2, x2, y2);
    }
    path_ellipse(id, x, y, rx, ry, rotation, startAngle, endAngle) {
        this.paths[id].ellipse(x, y, rx, ry, rotation, startAngle, endAngle);
    }
    path_line(id, x1, y1, x2, y2) {
        this.paths[id].moveTo(x1, x2);
        this.paths[id].lineTo(x2, y2);
    }
    path_rect(id, x, y, w, h) {
        this.paths[id].rect(x, y, w, h);
    }
    brush_color_new(id, color) {
        this.brushes[id] = colorIntToHex(color);
    }
    brush_linearGradient_new(id, x1, y1, x2, y2) {
        this.brushes[id] = this.ctx.createLinearGradient(x1, y1, x2, y2);
    }
    brush_addColorStop(id, offset, color) {
        this.brushes[id].addColorStop(offset, colorIntToHex(color));
    }
}
export const viewport = new Viewport();
//# sourceMappingURL=viewport.js.map