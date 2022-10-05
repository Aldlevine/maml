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
    init() {
        return __awaiter(this, void 0, void 0, function* () {
            wasm.setModuleImports("window.js", this);
            this.interop = yield wasm.getAssemblyExports("Maml.Wasm", "Maml.Window");
            window.onorientationchange =
                window.onresize = (_evt) => {
                    this.interop.HandleResize(0, window.innerWidth, window.innerHeight);
                    const canvas = document.getElementById("canvas");
                    canvas.width = window.innerWidth;
                    canvas.height = window.innerHeight;
                    canvas.style.width = `${window.innerWidth}px`;
                    canvas.style.height = `${window.innerHeight}px`;
                };
            window.onresize(null);
            window.onpointermove = (evt) => {
                this.interop.HandlePointerMove(0, evt.clientX, evt.clientY, evt.button, evt.buttons);
            };
            window.onpointerdown = (evt) => {
                this.interop.HandlePointerDown(0, evt.clientX, evt.clientY, evt.button, evt.buttons);
            };
            window.onpointerup = (evt) => {
                this.interop.HandlePointerUp(0, evt.clientX, evt.clientY, evt.button, evt.buttons);
            };
        });
    }
}
export const mamlWindow = new MamlWindow();
//# sourceMappingURL=window.js.map