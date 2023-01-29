import React from "react";
import { Row, Col, Form, Button } from "../external";
import DateTimeHelper from "../../utils/dateTimeHelper";
import "../../styles/component/bookmark.css";
import { useNavigate } from "react-router-dom";

export function Bookmark(props) {
    const navigate = useNavigate();

    return <div className="mb-3">
        <Row>
            <Col xs={1} className="d-flex justify-content-center align-items-center">
                <Form.Check onChange={(e) => {props.onBookmarkSelected(e.target.checked)}} checked={props.bookmark.isSelected} />
            </Col>
            <Col xs={11}>
                <a href="#" style={{textDecoration: "none"}}>{props.bookmark.title}</a>
                <div className="font-weight-normal">{props.bookmark.description.substring(0, 100)}</div>
                <div>
                    {props.bookmark.tags.map((tag) => {
                        return <Button variant="link" key={props.bookmark.id-tag.id} style={{textDecoration: "none"}} className="p-0 me-2">#{tag.name}</Button>
                    })}
                </div>
                <div>
                    <span>{DateTimeHelper.getFriendlyDate(props.bookmark.createdOn)} | </span>
                    <span>
                        <Button variant="link" className="p-0 me-2" style={{textDecoration: "none"}} onClick={() => navigate(`/bookmarks/${props.bookmark.id}`)}>Edit</Button>
                        <Button variant="link" className="p-0 me-2" style={{textDecoration: "none"}} onClick={props.onHideClicked}>{props.bookmark.isHidden ? "Unhide" : "Hide"}</Button>
                        <Button variant="link" className="text-danger p-0 me-2" style={{textDecoration: "none"}} onClick={props.onDeleteClicked}>Delete</Button>
                    </span>
                </div>
            </Col>
        </Row>
    </div>
}