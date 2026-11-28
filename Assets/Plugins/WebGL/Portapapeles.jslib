mergeInto(LibraryManager.library, {
  CopiarAlPortapapeles: function (textoPtr) {
    var texto = UTF8ToString(textoPtr);
    if (navigator.clipboard && navigator.clipboard.writeText) {
      navigator.clipboard.writeText(texto).catch(function () {
        prompt("Copia los datos (Ctrl+C):", texto);
      });
    } else {
      prompt("Copia los datos (Ctrl+C):", texto);
    }
  }
});