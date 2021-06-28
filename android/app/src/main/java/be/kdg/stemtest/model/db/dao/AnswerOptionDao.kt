package be.kdg.stemtest.model.db.dao

import androidx.room.*
import be.kdg.stemtest.model.entity.AnswerOption
import io.reactivex.Maybe
import io.reactivex.Single

@Dao
abstract class AnswerOptionDao {

    @Insert
    abstract fun insert(answerOption: AnswerOption)

    @Update
    abstract fun update(answerOption: AnswerOption)

    @Delete
    abstract fun delete(answerOption: AnswerOption)

    @Query("SELECT * FROM answeroption WHERE statementId =:statementId")
    abstract  fun getAnswerOptions(statementId:Int): Maybe<List<AnswerOption>>

    @Query("SELECT * FROM answeroption")
    abstract  fun getAllAnswerOptions(): Single<List<AnswerOption>>

    @Transaction
   open fun insertAllAnswerOptions(answerOptions:List<AnswerOption>, statementId: Int){
        for (answeroption in answerOptions) {
            answeroption.statementId=statementId
            insert(answeroption)
        }
    }

    @Transaction
   open fun updateAllAnswerOptions(answerOptions: List<AnswerOption>) {
        for (antwoordMogelijkheid in answerOptions) {
            update(antwoordMogelijkheid)
        }
    }

    @Query("SELECT * FROM answeroption WHERE correct=1")
     abstract fun getAllCorrectAnswerOptions() : Single<List<AnswerOption>>
}