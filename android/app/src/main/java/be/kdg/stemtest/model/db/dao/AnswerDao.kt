package be.kdg.stemtest.model.db.dao

import androidx.room.*
import be.kdg.stemtest.model.entity.StudentAnswer
import io.reactivex.Completable
import io.reactivex.Single

@Dao
interface AnswerDao {

    @Insert
    fun insert(antwoord:StudentAnswer) : Completable

    @Update
    fun update(antwoord:StudentAnswer) : Completable

    @Delete
    fun delete(antwoord:StudentAnswer)

    @Query("SELECT * FROM studentanswer ")
    fun getAllStudentAnswers(): Single<List<StudentAnswer>>

    @Query("SELECT * FROM studentanswer WHERE id=:id")
    fun getStudentAnswer(id:Int): Single<StudentAnswer>


}