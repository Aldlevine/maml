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
class MamlWindow {
    constructor() {
        this.canvas = document.getElementById("canvas");
        this.width = -1;
        this.height = -1;
        this.dpiRatio = -1;
    }
    init() {
        return __awaiter(this, void 0, void 0, function* () {
            wasm.setModuleImports("window.js", this);
            this.interop = yield wasm.getAssemblyExports("Maml.Wasm", "Maml.Window");
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
            window.oncontextmenu = (evt) => {
                evt.preventDefault();
            };
            window.onpointermove = (evt) => {
                this.interop.HandlePointerMove(0, evt.clientX, evt.clientY, evt.button, evt.buttons);
            };
            window.onpointerdown = (evt) => {
                this.interop.HandlePointerDown(0, evt.clientX, evt.clientY, evt.button, evt.buttons);
            };
            window.onpointerup = (evt) => {
                this.interop.HandlePointerUp(0, evt.clientX, evt.clientY, evt.button, evt.buttons);
            };
            this.updateSize();
        });
    }
    updateSize(force = false) {
        if (force || this.width != window.innerWidth || this.height != window.innerHeight || this.dpiRatio != window.devicePixelRatio) {
            this.width = window.innerWidth;
            this.height = window.innerHeight;
            this.dpiRatio = window.devicePixelRatio;
            this.canvas.width = Math.ceil(this.width * this.dpiRatio);
            this.canvas.height = Math.ceil(this.height * this.dpiRatio);
            this.canvas.style.width = `${this.width}px`;
            this.canvas.style.height = `${this.height}px`;
            this.interop.HandleResize(0, Math.ceil(this.width * this.dpiRatio), Math.ceil(this.height * this.dpiRatio), this.dpiRatio);
        }
    }
}
export const mamlWindow = new MamlWindow();
//# sourceMappingURL=window.js.map