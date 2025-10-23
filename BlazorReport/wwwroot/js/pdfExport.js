// PDFエクスポート機能

window.pdfExportFunctions = {
    /**
     * 社員データをPDFにエクスポートしてダウンロードします
     * @param {Array} employees - 社員データの配列
     * @param {string} fileName - ダウンロードするファイル名
     */
    exportToPdf: function (employees, fileName) {
        try {
            // jsPDFインスタンスの作成
            const { jsPDF } = window.jspdf;
            const doc = new jsPDF({
                orientation: 'landscape', // 横向き
                unit: 'mm',
                format: 'a4'
            });

            // 日本語フォントの設定（デフォルトフォントを使用）
            doc.setFont('helvetica');
            
            // タイトルの追加
            doc.setFontSize(16);
            doc.text('社員情報一覧', 14, 15);

            // テーブルのヘッダー定義
            const headers = [
                { header: '社員番号', dataKey: 'employeeNumber' },
                { header: '氏名', dataKey: 'name' },
                { header: '所属', dataKey: 'department' },
                { header: '役職', dataKey: 'post' },
                { header: '入社年月日', dataKey: 'dateOfJoining' }
            ];

            // データの整形（プロパティ名をキャメルケースに変換）
            const tableData = employees.map(emp => ({
                employeeNumber: emp.employeeNumber || '',
                name: emp.name || '',
                department: emp.department || '',
                post: emp.post || '',
                dateOfJoining: emp.dateOfJoining || ''
            }));

            // autoTableを使用してテーブルを生成
            doc.autoTable({
                columns: headers,
                body: tableData,
                startY: 25, // タイトルの下から開始
                styles: {
                    font: 'helvetica',
                    fontSize: 10,
                    cellPadding: 3,
                    overflow: 'linebreak',
                    halign: 'left'
                },
                headStyles: {
                    fillColor: [41, 128, 185], // 青色の背景
                    textColor: 255, // 白文字
                    fontStyle: 'bold',
                    halign: 'center'
                },
                alternateRowStyles: {
                    fillColor: [245, 245, 245] // 交互の行に薄いグレー
                },
                margin: { top: 25 }
            });

            // PDFをダウンロード
            doc.save(fileName || '社員情報.pdf');
            
            return true;
        } catch (error) {
            console.error('PDF export error:', error);
            return false;
        }
    }
};
