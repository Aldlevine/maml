class Renderer {
    canvas: HTMLCanvasElement;
    ctx: CanvasRenderingContext2D;


    Run(): void {
        this.canvas = document.getElementById("canvas") as HTMLCanvasElement;
        this.ctx = this.canvas.getContext("2d", { alpha: false, desynchronized: true, willReadFrequently: false });
        this.UpdateCanvasSize();

        window.addEventListener("resize", this.UpdateCanvasSize);
    }

    UpdateCanvasSize(): void {
        const scale: number = window.devicePixelRatio;
        const width : number = Math.ceil(window.innerWidth / 2) * 2;
        const height : number = Math.ceil(window.innerHeight / 2) * 2;
        this.canvas.width = Math.floor(width * scale);
        this.canvas.height = Math.floor(height * scale);
        this.canvas.style.width = `${width}px`;
        this.canvas.style.height = `${height}px`;
    }

    DrawLine(x0: number, y0: number, x1: number, y1: number, color: number = 0x00000000, thickness: number = 1) {
        this.ctx.beginPath();
        this.ctx.moveTo(x0, y0);
        this.ctx.lineTo(x1, y1);
        this.ctx.strokeStyle = Renderer.intToRgba(color);
        this.ctx.lineWidth = thickness;
        this.ctx.stroke();
    }

    private static intToRgba(int: number): string {
        const r = (int & 0xff000000) >>> 24;
        const g = (int & 0x00ff0000) >>> 16;
        const b = (int & 0x0000ff00) >>> 8;
        const a = (int & 0x000000ff) >> 0;
        return `rgba(${r}, ${g}, ${b}, ${a / 255})`;
    }
}
