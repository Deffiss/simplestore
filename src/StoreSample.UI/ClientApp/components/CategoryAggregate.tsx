import * as React from 'react';
import { ICategoryAggregate } from '../models/ICategoryAggregate';
import { NavLink } from 'react-router-dom';

interface Props {
  categoryAggregate: ICategoryAggregate
}

export class CategoryAggregate extends React.Component<Props, {}> {
  public render() {
    const { categoryAggregate } = this.props;
    return <li>
      <NavLink exact to={`/${categoryAggregate.id}`} activeClassName='active'>
        <span className="aggr__name">{categoryAggregate.name}</span>
        <span className="aggr__count">{categoryAggregate.productCount}</span>
      </NavLink>
    </li>
  }
}