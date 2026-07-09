function getCurrentAllocationStatementByTypeAndYear(type, year) {
    var collection = getContext().getCollection();

    var query = {
        query: "SELECT TOP 1 * FROM c WHERE c.name = @Type AND c.year = @Year ORDER BY c.version desc",
        parameters: [
            { name: "@Type", value: type },
            { name: "@Year", value: year }
        ]
    };

    var isAccepted = collection.queryDocuments(
        collection.getSelfLink(),
        query,

        function (err, feed) {
            if (err) throw err;

            var response = getContext().getResponse();
            response.setBody(feed.length === 1 ? feed[0] : null);
        });

    if (!isAccepted) throw new Error('The query was not accepted by the server.');
}