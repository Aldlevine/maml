var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import createDotnetRuntime from "./dotnet.js";
class Wasm {
    constructor() {
        // public
        this._exports = {};
    }
    init() {
        return __awaiter(this, void 0, void 0, function* () {
            try {
                const { getAssemblyExports, setModuleImports, runMain } = yield createDotnetRuntime({});
                this._getAssemblyExports = getAssemblyExports;
                this._runMain = runMain;
                this._setModuleImports = setModuleImports;
            }
            catch (err) {
                console.error(`WASM ERROR ${err}`);
            }
        });
    }
    getAssemblyExports(assemblyName, qualifiedName) {
        var _a;
        var _b;
        return __awaiter(this, void 0, void 0, function* () {
            const assembly = (_a = (_b = this._exports)[assemblyName]) !== null && _a !== void 0 ? _a : (_b[assemblyName] = yield this._getAssemblyExports(assemblyName));
            const path = qualifiedName.trim().split(".");
            let namespace = assembly;
            for (let p of path) {
                namespace = namespace[p];
            }
            return namespace;
        });
    }
    setModuleImports(moduleName, moduleImports) {
        return this._setModuleImports(moduleName, moduleImports);
    }
    run(mainAssembly, ...args) {
        return __awaiter(this, void 0, void 0, function* () {
            return this._runMain(mainAssembly, args);
        });
    }
}
export const wasm = new Wasm();
//# sourceMappingURL=wasm.js.map