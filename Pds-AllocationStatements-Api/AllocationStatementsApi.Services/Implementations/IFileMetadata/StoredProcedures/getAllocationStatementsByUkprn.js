function getAllocationStatementsByUkprn() {
    var collection = getContext().getCollection();

    var  isAccepted = collection.queryDocuments(
        collection.getSelfLink(),
        'SELECT * from c',

    function  (err, feed) {
        if (err) throw err;

        var response = getContext().getResponse();
        response.setBody(feed);
    });

if (!isAccepted) throw new Error('The query was not accepted by the server.');
}