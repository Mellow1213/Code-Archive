#include "Queue.h"

Queue::Queue(int s)
{
    capacity_ = s;
    arr = new int[s];
    size_ = 0;
    front_ = 0;
    rear_ = 0;
}

Queue::~Queue()
{
    delete[] arr;
}

void Queue::push(int e)
{
    if (size_ >= capacity_)
        reserve(capacity_ * 2);
    arr[rear_] = e;
    moveNext(rear_);
    size_++;
}

int Queue::pop()
{
    if (size_ <= 0)
        return -1;
    int back = arr[front_];
    moveNext(front_);
    size_--;
    return back;
}

int Queue::size()
{
    return size_;
}

bool Queue::empty()
{
    return size_ <= 0;
}

int Queue::front()
{
    return (empty() ? -1 : arr[front_]);
}

int Queue::back()
{
    if (size_ <= 0)
        return -1;                                   // 비어있으면 -1 반환
    return arr[(rear_ - 1 + capacity_) % capacity_]; // 원형 큐 고려
}

void Queue::reserve(int cap)
{
    if (cap > capacity_)
    {
        int *new_arr = new int[cap];
        // 현재 front_부터 시작해서 size_만큼 순차적으로 복사
        for (int i = 0; i < size_; i++)
        {
            new_arr[i] = arr[(front_ + i) % capacity_];
        }
        delete[] arr;
        arr = new_arr;
        front_ = 0;
        rear_ = size_; // rear_는 다음 삽입 위치이므로 size_
        capacity_ = cap;
    }
}
