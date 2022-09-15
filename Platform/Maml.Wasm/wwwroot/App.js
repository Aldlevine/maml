class Input {
    Run() {
        const resizeHandler = window.Module.mono_bind_static_method("[Maml.Wasm] Maml.Program:ResizeHandler");
        window.addEventListener("resize", () => {
            resizeHandler(window.innerWidth, window.innerHeight);
        });
    }
}
class Renderer {
    Run() {
        this.canvas = document.getElementById("canvas");
        this.ctx = this.canvas.getContext("2d", { alpha: false, desynchronized: true, willReadFrequently: false });
        this.UpdateCanvasSize();
        window.addEventListener("resize", this.UpdateCanvasSize);
    }
    UpdateCanvasSize() {
        const scale = window.devicePixelRatio;
        const width = Math.ceil(window.innerWidth / 2) * 2;
        const height = Math.ceil(window.innerHeight / 2) * 2;
        this.canvas.width = Math.floor(width * scale);
        this.canvas.height = Math.floor(height * scale);
        this.canvas.style.width = `${width}px`;
        this.canvas.style.height = `${height}px`;
    }
    DrawLine(x0, y0, x1, y1, color = 0x00000000, thickness = 1) {
        this.ctx.beginPath();
        this.ctx.moveTo(x0, y0);
        this.ctx.lineTo(x1, y1);
        this.ctx.strokeStyle = Renderer.intToRgba(color);
        this.ctx.lineWidth = thickness;
        this.ctx.stroke();
    }
    static intToRgba(int) {
        const r = (int & 0xff000000) >>> 24;
        const g = (int & 0x00ff0000) >>> 16;
        const b = (int & 0x0000ff00) >>> 8;
        const a = (int & 0x000000ff) >> 0;
        return `rgba(${r}, ${g}, ${b}, ${a / 255})`;
    }
}
/// <reference path="Input.ts" />
/// <reference path="Renderer.ts" />
const input = new Input();
window.Input = input;
const renderer = new Renderer();
window.Renderer = renderer;
input.Run();
//# sourceMappingURL=App.js.map