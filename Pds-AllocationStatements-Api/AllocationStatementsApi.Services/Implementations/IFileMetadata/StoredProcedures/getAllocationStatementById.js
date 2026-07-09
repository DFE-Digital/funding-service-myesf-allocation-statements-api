function getAllocationStatementById(id) {
    var collection = getContext().getCollection();

    var query = {
        query: "SELECT * from c WHERE c.id = @ID",
        parameters: [
            { name: "@ID", value: id }
        ]
    };

    var isAccepted = collection.queryDocuments(
        collection.getSelfLink(),
        query,

        function (err, feed) {
            if (err) throw err;

            var response = getContext().getResponse();
            response.setBody(feed);
        });

    if (!isAccepted) throw new Error('The query was not accepted by the server.');
}