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
        this.contexts[id].clearRect(0, 0, canvas.width, canvas.height);
        document.body.style.background = color;
    }
}
export const renderTarget = new RenderTarget();
//# sourceMappingURL=render-target.js.map