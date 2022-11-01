import { wasm } from "./maml/wasm.js";
import { renderTarget } from "./maml/render-target.js";
import { mamlWindow } from "./maml/window.js";
import { engine } from "./maml/engine.js";

Main();

async function Main() {
	window.onunhandledrejection = (p) => {
		console.error(p.reason);
		PresentError();
	};
	window.onerror = (e, source, line, col, error) => {
		console.error(error);
		PresentError();
	};

	try {
		await wasm.init();
		await renderTarget.init();
		await mamlWindow.init();
		await engine.init();
		await wasm.run("BasicLayout.Wasm");
	}
	catch (e)
	{
		console.error(e);
		PresentError();
	}
}

function PresentError() {
	document.getElementById("canvas").style.display = "none";
	document.getElementById("error").style.display = "inline-block";
}
