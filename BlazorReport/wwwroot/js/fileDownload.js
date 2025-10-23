// ファイルダウンロード機能

window.fileDownloadFunctions = {
    /**
     * バイト配列からBlobを生成してファイルをダウンロードします
     * @param {string} fileName - ダウンロードするファイル名
     * @param {string} contentType - ファイルのMIMEタイプ
     * @param {Uint8Array} byteArray - ファイルのバイト配列
     */
    downloadFile: function (fileName, contentType, byteArray) {
        try {
            // バイト配列からBlobを作成
            const blob = new Blob([byteArray], { type: contentType });
            
            // ダウンロード用のリンクを作成
            const link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = fileName;
            
            // リンクをクリックしてダウンロード
            document.body.appendChild(link);
            link.click();
            
            // リンクを削除してメモリを解放
            document.body.removeChild(link);
            window.URL.revokeObjectURL(link.href);
            
            return true;
        } catch (error) {
            console.error('File download error:', error);
            return false;
        }
    }
};
