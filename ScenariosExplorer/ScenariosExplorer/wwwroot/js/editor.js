var editor = null;

require.config({
    baseUrl: 'https://microsoft.github.io/monaco-editor/node_modules/monaco-editor/min/'
});

$(document).ready(function () {
    require(['vs/editor/editor.main'], function () {
        loadEditor();
    });

    window.onresize = function () {
        if (editor) {
            editor.layout();
        }
    };
});

function loadEditor() {
    if (!editor) {
        $('#editor').empty();
        editor = monaco.editor.create(document.getElementById('editor'), {
            model: null,
            wordWrap: "on"
        });
    }

    var oldModel = editor.getModel();
    var newModel = monaco.editor.createModel($("#HiddenContents").val(), "markdown");
    editor.setModel(newModel);
    if (oldModel) {
        oldModel.dispose();
    }
    $('.loading.editor').fadeOut({ duration: 300 });
}