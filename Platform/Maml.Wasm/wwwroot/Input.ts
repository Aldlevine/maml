class Input {
    Run() {
        const resizeHandler = (window as any).Module.mono_bind_static_method("[Maml.Wasm] Maml.Program:ResizeHandler");
        window.addEventListener("resize", () => {
            resizeHandler(window.innerWidth, window.innerHeight);
        });
    }
}
