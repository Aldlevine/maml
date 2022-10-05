import { wasm } from "./wasm.js";

type EngineInterop = {
	HandleAnimationFrame: (id: number) => void;
}

class Engine {

	private interop: EngineInterop;

	public async init(): Promise<void> {
		wasm.setModuleImports("engine.js", this);
		this.interop = await wasm.getAssemblyExports<EngineInterop>("Maml.Wasm", "Maml.Engine");
		requestAnimationFrame(this.boundFrameRequestCallback);
	}

	private frameRequestCallback(): void {
		this.interop.HandleAnimationFrame(0);
		requestAnimationFrame(this.boundFrameRequestCallback);
	}

	private boundFrameRequestCallback = this.frameRequestCallback.bind(this);
}

export const engine = new Engine();
