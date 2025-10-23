// HTML2Canvas を使用した日本語対応PDFエクスポート

/**
 * HTML2Canvasを使用して社員データをPDFにエクスポートします（日本語対応）
 * @param {Array} employees - 社員データの配列
 * @param {string} fileName - ダウンロードするファイル名
 * @returns {Promise<boolean>} 成功した場合true、失敗した場合false
 */
window.pdfExportFunctions.exportToPdfCanvas = async function (employees, fileName) {
    try {

        // 一時的なHTML要素を作成
        const container = document.createElement('div');
        container.style.position = 'absolute';
        container.style.left = '-9999px';
        container.style.background = 'white';
        container.style.padding = '20px';
        container.style.fontFamily = 'Arial, sans-serif';
        
        // タイトル
        const title = document.createElement('h2');
        title.textContent = '社員情報一覧';
        title.style.marginBottom = '20px';
        container.appendChild(title);

        // テーブル作成
        const table = document.createElement('table');
        table.style.width = '100%';
        table.style.borderCollapse = 'collapse';
        table.style.fontSize = '12px';

        // ヘッダー
        const thead = document.createElement('thead');
        const headerRow = document.createElement('tr');
        ['社員番号', '氏名', '所属', '役職', '入社年月日'].forEach(text => {
            const th = document.createElement('th');
            th.textContent = text;
            th.style.border = '1px solid #000';
            th.style.padding = '8px';
            th.style.backgroundColor = '#2980b9';
            th.style.color = 'white';
            th.style.fontWeight = 'bold';
            headerRow.appendChild(th);
        });
        thead.appendChild(headerRow);
        table.appendChild(thead);

        // データ行
        const tbody = document.createElement('tbody');
        employees.forEach((emp, index) => {
            const row = document.createElement('tr');
            if (index % 2 === 1) {
                row.style.backgroundColor = '#f5f5f5';
            }
            
            const values = [
                emp['社員番号'] || emp.employeeNumber || emp.EmployeeNumber || '',
                emp['氏名'] || emp.name || emp.Name || '',
                emp['所属'] || emp.department || emp.Department || '',
                emp['役職'] || emp.post || emp.Post || '',
                emp['入社年月日'] || emp.dateOfJoining || emp.DateOfJoining || ''
            ];

            values.forEach(value => {
                const td = document.createElement('td');
                td.textContent = value;
                td.style.border = '1px solid #000';
                td.style.padding = '6px';
                row.appendChild(td);
            });
            
            tbody.appendChild(row);
        });
        table.appendChild(tbody);
        container.appendChild(table);

        // DOMに追加（画面外に配置）
        document.body.appendChild(container);

        // html2canvasでHTMLを画像化
        const canvas = await html2canvas(container, {
            scale: 2,
            logging: false,
            useCORS: true,
            allowTaint: true
        });

        // DOMから削除
        document.body.removeChild(container);

        // CanvasからPDFを生成
        const { jsPDF } = window.jspdf;
        const imgData = canvas.toDataURL('image/png');
        
        const pdf = new jsPDF({
            orientation: 'landscape',
            unit: 'mm',
            format: 'a4'
        });

        const imgWidth = 277; // A4 landscape width in mm (minus margins)
        const pageHeight = 190; // A4 landscape height in mm (minus margins)
        const imgHeight = (canvas.height * imgWidth) / canvas.width;
        
        pdf.addImage(imgData, 'PNG', 10, 10, imgWidth, Math.min(imgHeight, pageHeight));

        // PDFをダウンロード
        pdf.save(fileName || '社員情報.pdf');

        return true;
    } catch (error) {
        console.error('PDF export (Canvas method) error:', error);
        return false;
    }
};
