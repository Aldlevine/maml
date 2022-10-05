import { wasm } from "./wasm.js";
import { renderTarget } from "./render-target.js";
import { mamlWindow } from "./window.js";
import { engine } from "./engine.js";

Main();

async function Main() {
	await wasm.init();
	await renderTarget.init();
	await mamlWindow.init();
	await engine.init();
	await wasm.run("Maml.Wasm");
}
