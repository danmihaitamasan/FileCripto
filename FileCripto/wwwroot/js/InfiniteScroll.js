let infinite_scroll = new function () {
    let self = this;
    let InfinityScrollUtils = {
        ticking: false,
        page: 2,
        getScrollPosition: function () {
            let s = $(window).scrollTop(),
                d = $(document).height(),
                c = $(window).height();


            return (s / (d - c)) * 100;
        },
        generateNewTableRow: function (item) {
            let tableRow = document.createElement('tr');
            let fileNameTD = document.createElement('td');
            let folderNameTD = document.createElement('td');
            let DetailsTD = document.createElement('td');

            fileNameTD.innerText = item.fileName;
            tableRow.appendChild(fileNameTD);

            folderNameTD.innerText = item.folderName;
            tableRow.appendChild(folderNameTD);

            let details = document.createElement('button');
            details.onclick = function (e) {
                window.location.href = `/FileManager/CheckDownload/${item.fileId}`;
            }
            details.textContent = "Download";
            details.className = "btn btn-primary"
            DetailsTD.appendChild(details);
            tableRow.appendChild(DetailsTD);
            return tableRow;
        }
    }


    self.InfinityScroll = () => {
        if (InfinityScrollUtils.getScrollPosition() > 30) {
            if (!InfinityScrollUtils.ticking) {
                $.ajax({
                    url: "/FileManager/GetOwnFilesPerPage?page=" + InfinityScrollUtils.page,
                }).done(function (data) {
                    const tableBody = $("#tabel>tbody");
                    data.forEach(function (item) {
                        tableBody.append(InfinityScrollUtils.generateNewTableRow(item));
                    })
                    InfinityScrollUtils.page = InfinityScrollUtils.page + 1;
                    InfinityScrollUtils.ticking = false;
                }).fail(function (xhr, textStatus, errorThrown) {
                    debugger;
                    console.log('Bad stuff happened');
                    InfinityScrollUtils.ticking = false;
                })
                InfinityScrollUtils.ticking = true;


            }
        }
    }
};














