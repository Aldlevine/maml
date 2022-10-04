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
import { mamlWindow } from "./window.js";
//import { viewport } from "./viewport.js";
//import { input } from "./input.js";
Main();
function Main() {
    return __awaiter(this, void 0, void 0, function* () {
        yield wasm.init();
        //await viewport.init();
        //await input.init();
        yield mamlWindow.init();
        yield wasm.run("Maml.Wasm");
    });
}
//# sourceMappingURL=main.js.map