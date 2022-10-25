import { wasm } from "./maml/wasm.js";
import { renderTarget } from "./maml/render-target.js";
import { mamlWindow } from "./maml/window.js";
import { engine } from "./maml/engine.js";

Main();

async function Main() {
	window.onunhandledrejection = PresentError;
	window.onerror = PresentError;

	try {
		await wasm.init();
		await renderTarget.init();
		await mamlWindow.init();
		await engine.init();
		await wasm.run("KitchenSink.Wasm");
	}
	catch (e)
	{
		PresentError();
	}
}

function PresentError() {
	document.getElementById("canvas").style.display = "none";
	document.getElementById("error").style.display = "inline-block";
}
