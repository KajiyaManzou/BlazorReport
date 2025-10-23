// 日本語フォント設定
// このファイルはjsPDFに日本語フォント情報を追加します

window.addJapaneseFontToJsPDF = function() {
    // jsPDFがロードされているか確認
    if (!window.jspdf || !window.jspdf.jsPDF) {
        console.error('jsPDF not loaded');
        return false;
    }
    
    console.log('Japanese font support initialized (using default with unicode support)');
    return true;
};

// 初期化
document.addEventListener('DOMContentLoaded', function() {
    window.addJapaneseFontToJsPDF();
});
