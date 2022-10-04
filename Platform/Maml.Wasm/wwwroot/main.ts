import { wasm } from "./wasm.js";
import { mamlWindow } from "./window.js";
//import { viewport } from "./viewport.js";
//import { input } from "./input.js";

Main();

async function Main() {
	await wasm.init();
	//await viewport.init();
	//await input.init();
	await mamlWindow.init();
	await wasm.run("Maml.Wasm");
}
