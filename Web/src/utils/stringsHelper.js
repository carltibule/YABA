// https://stackoverflow.com/questions/29652862/highlight-text-using-reactjs
export const getHighlightedText = (text, highlight) => {
    // Split on highlight term and include term into parts, ignore case
    const parts = text.split(new RegExp(`(${highlight})`, 'gi'));
    return <span> { parts.map((part, i) => 
        <span key={i} className={part.toLowerCase() === highlight.toLowerCase() ? "fw-bold" : ""}>
            { part }
        </span>)
    } </span>;
}