import { wasm } from "./wasm.js";
import { viewport } from "./viewport.js";
import { input } from "./input.js";
Main();
async function Main() {
    await wasm.init();
    await viewport.init();
    await input.init();
    await wasm.run("Maml.Wasm");
}
//# sourceMappingURL=main.js.map