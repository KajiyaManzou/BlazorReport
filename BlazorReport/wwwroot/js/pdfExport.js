// PDFエクスポート機能

window.pdfExportFunctions = {
    /**
     * 社員データをPDFにエクスポートしてダウンロードします
     * @param {Array} employees - 社員データの配列
     * @param {string} fileName - ダウンロードするファイル名
     */
    exportToPdf: function (employees, fileName) {
        try {
            console.log('PDF export started');
            console.log('Employees count:', employees?.length);
            console.log('Employees data:', employees);
            if (employees && employees.length > 0) {
                console.log('First employee:', employees[0]);
                console.log('First employee keys:', Object.keys(employees[0]));
            }
            console.log('jsPDF available:', typeof window.jspdf);
            console.log('autoTable available:', typeof window.jspdf?.jsPDF?.prototype?.autoTable);

            // jsPDFインスタンスの作成
            if (!window.jspdf || !window.jspdf.jsPDF) {
                throw new Error('jsPDF library not loaded');
            }

            const { jsPDF } = window.jspdf;
            const doc = new jsPDF({
                orientation: 'landscape', // 横向き
                unit: 'mm',
                format: 'a4',
                putOnlyUsedFonts: true,
                compress: true
            });

            // autoTableが利用可能か確認
            if (typeof doc.autoTable !== 'function') {
                throw new Error('jsPDF-AutoTable plugin not loaded or not integrated');
            }

            // 日本語対応の設定
            // jsPDF 2.5.1+ では標準フォントでもある程度の日本語表示が可能
            doc.setLanguage('ja');

            // タイトルの追加
            doc.setFontSize(16);
            doc.text('社員情報一覧', 14, 15);

            // テーブルのヘッダー定義（配列形式で指定、日本語）
            const headers = [
                ['社員番号', '氏名', '所属', '役職', '入社年月日']
            ];

            // データの整形（配列の配列形式に変換）
            // 日本語プロパティ名に対応
            const tableData = employees.map(emp => {
                const row = [
                    emp['社員番号'] || emp.employeeNumber || emp.EmployeeNumber || '',
                    emp['氏名'] || emp.name || emp.Name || '',
                    emp['所属'] || emp.department || emp.Department || '',
                    emp['役職'] || emp.post || emp.Post || '',
                    emp['入社年月日'] || emp.dateOfJoining || emp.DateOfJoining || ''
                ];
                console.log('Row data:', row, 'from employee:', emp);
                return row;
            });

            console.log('Table data (array format):', tableData);
            console.log('Table data count:', tableData.length);

            // autoTableを使用してテーブルを生成
            doc.autoTable({
                head: headers,
                body: tableData,
                startY: 25, // タイトルの下から開始
                styles: {
                    font: 'helvetica',
                    fontSize: 10,
                    cellPadding: 3,
                    overflow: 'linebreak',
                    halign: 'left',
                    minCellHeight: 10
                },
                headStyles: {
                    fillColor: [41, 128, 185], // 青色の背景
                    textColor: [255, 255, 255], // 白文字
                    fontStyle: 'bold',
                    halign: 'center',
                    fontSize: 11
                },
                bodyStyles: {
                    fontSize: 10,
                    textColor: [0, 0, 0]
                },
                alternateRowStyles: {
                    fillColor: [245, 245, 245] // 交互の行に薄いグレー
                },
                margin: { top: 25 },
                theme: 'grid',
                // 日本語を含むテキストの処理
                didParseCell: function(data) {
                    // セル内のテキストをUnicode対応で処理
                    if (data.cell.raw && typeof data.cell.raw === 'string') {
                        data.cell.text = [data.cell.raw];
                    }
                }
            });

            // PDFをダウンロード
            console.log('Saving PDF as:', fileName);
            doc.save(fileName || '社員情報.pdf');

            console.log('PDF export completed successfully');
            return true;
        } catch (error) {
            console.error('PDF export error:', error);
            return false;
        }
    }
};
