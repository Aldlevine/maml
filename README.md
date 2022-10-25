# Maml

***Maml Ain't a Markup Language*** - A .Net based multiplatform UI library.

---

Maml is currently in active development and far from production ready at the moment. However, though incomplete, many of the fundamentals have been developed to **Good Enough^TM^** standards, and functional "applications" can be produced which run on both Windows and Web.

---

## Design

Maml is a retained mode UI library which provides an abstraction layer on top of platform-native APIs.

Unlike Blazor hybrid (and similar), Maml focuses on producing high performance native applications without reliance on web technologies, while at the same time targeting web with a more "native-esque" approach. In this way we can produce from a single codebase both a web apps and native apps without sacrificing performance on native platforms.

Unlike Uno Platform, Maml is provides a "from ground up" API. It is not a port of existing Windows UI libraries to other platforms. Furthermore, rather than "reskinning" existing native UI components, Maml builds on low-level drawing / input / etc... APIs. Once the low level APIs are hooked up for a given platform, all of the platform agnostic code (ideally the majority of the codebase) should **Just Work^TM^**.

## Future

In order to get bootstrapped, we are simultaneously developing backends for both Windows (Direct2D) and Web (HTML/WASM). Once we've largely stabilized the API, additional platform backends will be developed, including for Linux, Mac, Android, and iOS.

Audio playback is not out of scope for Maml, but is currently taking a backseat to graphics and rendering.

While we are currently developing against existing 2D drawing APIs (Direct2D, CanvasRenderingContext2D), which are generally immediate mode, in the future we intend to develop a custom retained mode 2D drawing library that better conforms to the abstract API. This will ideally provide both a performance boost and the capacity to develop more compelling visuals without sacrificing portability.
