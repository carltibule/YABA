export const containsSubstring = (bookmark, searchString) => {
    if (bookmark === null) {
        return false;
    }
    
    return Object.values(bookmark)
        .some(val => typeof(val) == 'object' ?
            containsSubstring(val, searchString) :
                typeof(val) == 'number' || typeof(val) == 'boolean' ?
                    val.toString().toLowerCase().includes(searchString.toLowerCase()) : 
                    val.toLowerCase().includes(searchString.toLowerCase())
        );
}