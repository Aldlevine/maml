import createDotnetRuntime from "./dotnet.js";

class Wasm {
// public

	public async init() : Promise<void> {
		try {
			const { getAssemblyExports, setModuleImports, runMain } = await createDotnetRuntime({});
			this._getAssemblyExports = getAssemblyExports;
			this._runMain = runMain;
			this._setModuleImports = setModuleImports;
		}
		catch (err) {
			console.error(`WASM ERROR ${err}`);
		}
	}

	public async getAssemblyExports<T>(assemblyName: string, qualifiedName: string): Promise<T> {
		const assembly = this._exports[assemblyName] ??= await this._getAssemblyExports(assemblyName);
		const path = qualifiedName.trim().split(".");
		let namespace = assembly;
		for (let p of path) {
			namespace = namespace[p];
		}
		return namespace;
	}

	public setModuleImports<T>(moduleName: string, moduleImports: T): void {
		return this._setModuleImports(moduleName, moduleImports);
	}

	public async run(mainAssembly: string, ...args: string[]): Promise<number> {
		return this._runMain(mainAssembly, args);
	}

// private

	// fn pointers
	private _getAssemblyExports: (assemblyName: string) => Promise<any>;
	private _setModuleImports: (moduleName: string, moduleImports: any) => void;
	private _runMain: (assemblyName: string, args: string[]) => Promise<number>;

	private _exports: { [key: string]: any } = {};
}

export const wasm = new Wasm();
