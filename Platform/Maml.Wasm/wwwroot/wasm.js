import createDotnetRuntime from "./dotnet.js";
class Wasm {
    constructor() {
        // public
        this._exports = {};
    }
    async init() {
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
    async getAssemblyExports(assemblyName, qualifiedName) {
        var _a;
        var _b;
        const assembly = (_a = (_b = this._exports)[assemblyName]) !== null && _a !== void 0 ? _a : (_b[assemblyName] = await this._getAssemblyExports(assemblyName));
        const path = qualifiedName.trim().split(".");
        let namespace = assembly;
        for (let p of path) {
            namespace = namespace[p];
        }
        return namespace;
    }
    setModuleImports(moduleName, moduleImports) {
        return this._setModuleImports(moduleName, moduleImports);
    }
    async run(mainAssembly, ...args) {
        return this._runMain(mainAssembly, args);
    }
}
export const wasm = new Wasm();
//# sourceMappingURL=wasm.js.map