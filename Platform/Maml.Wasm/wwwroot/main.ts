import { wasm } from "./wasm.js";
import { input } from "./input.js";
import { renderer } from "./renderer.js";

Main();

async function Main() {
	await wasm.init();
	await input.init();
	await renderer.init();
	await wasm.run("Maml.Wasm");
}
