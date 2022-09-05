let MySelect2 = new function () {
    let self = this;
    self.MySelect2 = (searchName, searchOption, placeHolderName) => {
        $(searchName).select2({
            ajax: {
                url: searchOption,
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    return {
                        search: params.term,
                        page: params.page
                    };
                },
                processResults: function (data, params) {
                    data.forEach(i => {
                        i.id = i.value
                    });
                    params.page = params.page || 1;
                    return {
                        results: data
                    };
                },
                cache: true
            },
            placeholder: 'Search for a ' + placeHolderName,
            minimumInputLength: 1,
            templateResult: formatItem,
            templateSelection: formatRepoSelection
        });

        function formatItem(repo) {
            let $container = $(
                "<div class='select2-result-repository clearfix'>" +

                "<div class='select2-result-repository__title'></div>" +

                "</div>"
            );

            $container.find(".select2-result-repository__title").text(repo.text);

            return $container;
        }

        function formatRepoSelection(repo) {
            return repo.full_name || repo.text;
        }
    }
};