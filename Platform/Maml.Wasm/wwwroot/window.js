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
            const interop = yield wasm.getAssemblyExports("Maml.Wasm", "Maml.Window");
            window.onresize = (ev) => {
                interop.HandleResize(0, window.innerWidth, window.innerHeight);
            };
            window.onpointermove = (ev) => {
                interop.HandlePointerMove(0, ev.clientX, ev.clientY, ev.button, ev.buttons);
            };
            window.onpointerdown = (ev) => {
                interop.HandlePointerDown(0, ev.clientX, ev.clientY, ev.button, ev.buttons);
            };
            window.onpointerup = (ev) => {
                interop.HandlePointerUp(0, ev.clientX, ev.clientY, ev.button, ev.buttons);
            };
        });
    }
}
export const mamlWindow = new MamlWindow();
//# sourceMappingURL=window.js.map