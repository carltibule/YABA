export const getTagGroups = (allBookmarkTags) => {
    const allTags = allBookmarkTags.flat().reduce((accumulator, current) => {
        if(!accumulator.find((item) => item.id === current.id)) {
            accumulator.push(current);
        }

        return accumulator
    }, []);

    return allTags.map((x) => x['name'][0].toUpperCase()).reduce((accumulator, current) => {
        if(!accumulator.find((item) => item.name === current)) {
            accumulator.push({name: current, tags: allTags.filter((x) => x.name[0].toUpperCase() === current)});
        }
        
        return accumulator
    }, []).sort((a, b) => (a.name < b.name) ? -1 : (a.name > b.name) ? 1 : 0);
}