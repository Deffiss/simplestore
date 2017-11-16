import { ICategoryAggregate } from '../models/ICategoryAggregate';
import { ICategory } from '../models/ICategory';
import { IProduct } from '../models/IProduct';
import { IProductCommand } from '../models/IProductCommand';

import axios from 'axios';

const COMMANDS_BASE_URL = 'http://localhost:5001/api/v1/';
const QUERIES_BASE_URL = 'http://localhost:5002/api/v1/';

export class CatalogService {
  getCategoryAggregates(): Promise<ICategoryAggregate[]> {
    return new Promise<ICategoryAggregate[]>((resolve, reject) => {
      axios.get<ICategoryAggregate[]>(`${QUERIES_BASE_URL}categories/aggregates`).then(
        res => resolve(res.data)
      )
      .catch(reject);
    })
  }
  getProducts(categoryId: string): Promise<IProduct[]> {
    return new Promise<IProduct[]>((resolve, reject) => {
      axios.get<IProduct[]>(`${QUERIES_BASE_URL}products/categories/${categoryId}/products`).then(
        res => resolve(res.data)
      )
      .catch(reject)
    })
  }
  getCategories(): Promise<ICategory[]> {
    return new Promise<ICategory[]>((resolve, reject) => {
      axios.get<ICategory[]>(`${QUERIES_BASE_URL}categories`).then(
        res => resolve(res.data)
      )
      .catch(reject);
    })
  }
  getCategory(categoryId: string): Promise<ICategory> {
    return new Promise<ICategory>((resolve, reject) => {
      axios.get<ICategory>(`${QUERIES_BASE_URL}categories/${categoryId}`).then(
        res => resolve(res.data)
      )
      .catch(reject);
    })
  }
  postProduct(productCommand: IProductCommand): Promise<{}> {
    return new Promise<{}>((resolve, reject) => {
      axios.post(`${COMMANDS_BASE_URL}products`, productCommand).then(resolve).catch(reject);
    })
  }
}

export default new CatalogService();