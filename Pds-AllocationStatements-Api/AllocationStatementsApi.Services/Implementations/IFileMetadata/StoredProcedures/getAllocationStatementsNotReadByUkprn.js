function getAllocationStatementsNotReadByUkprn(principal) {
    var collection = getContext().getCollection();

    var query = {
        query: "SELECT value count(1) from c WHERE ARRAY_CONTAINS(c.history, {Action: \"ViewedInDetail\", User: { Principle: @Principal }}, true) = false",
        parameters: [
            { name: "@Principal", value: principal }
        ]
    };

    var isAccepted = collection.queryDocuments(
        collection.getSelfLink(),
        query,

        function (err, feed) {
            if (err) throw err;

            var response = getContext().getResponse();
            response.setBody(feed[0]);
        });

    if (!isAccepted) throw new Error('The query was not accepted by the server.');
}