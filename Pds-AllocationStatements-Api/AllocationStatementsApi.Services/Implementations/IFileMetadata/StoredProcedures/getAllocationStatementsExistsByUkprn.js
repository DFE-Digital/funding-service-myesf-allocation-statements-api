function getAllocationStatementsExistsByUkprn() {
    var collection = getContext().getCollection();

    var isAccepted = collection.queryDocuments(
        collection.getSelfLink(),
        "SELECT value count(1) from c",

        function (err, feed) {
            if (err) throw err;

            var response = getContext().getResponse();
            response.setBody(feed[0] > 0);
        });

    if (!isAccepted) throw new Error('The query was not accepted by the server.');
}